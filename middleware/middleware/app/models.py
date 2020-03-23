from typing import List
from pydantic import BaseModel, validator
from pydantic import HttpUrl


# REQUEST OBJECT STRUCTURE #
class RequestStructure(BaseModel):
    """
        **Request Structure**

        Middleware expects a list of microsites, float user latitude and a float user longitude.
        pyDantic specifies the default types of each field. If the fields sent in the request cannot be processed as the
        types that the middleware is expecting then a 422 unprocessable entity status code is raised.

    """
    microsites: list
    latitude: float
    longitude: float

    @validator('microsites')
    def should_contain_one_microsite(cls, microsites):
        """
            **Validation of microsites in request**

            The list should contain microsites. If the length of the provided list is 0, or the first index of the list is None
            we raise a ValueError.

        """
        if len(microsites) == 0 or microsites[0] is None:
            raise ValueError("There should be atleast one microsite")
        return microsites


# RESPONSE OBJECT STRUCTURE #
class SlotStructure(BaseModel):
    """
        **Slot Structure**

        A single slot is a dictionary, containing the key 'time' and the key 'promotions'.
        Accessed with key 'time' should give an ISO 8601 str and accessing with key 'promotions' should give a list.

    """
    time: str
    promotions: List[str]


class ReviewStructure(BaseModel):
    """
        **Review Structure**

        Reviews should be a JSON. With key 'average' giving a float, and key 'count' giving an int.
    """
    average: float
    count: int


    @validator('average', 'count', pre=True)

    def verify_not_null_value(cls, value):
        """
            **Validator on reviews**

            We validate both 'average' and 'count'. If either of these are not a float or an int, we set its value to 0
            and return the current value. Otherwise, we return its current value.

            :param value: Either average or count

            :return: Value of parameter. Either original value or 0
            :rtype: int or float

        """
        if not isinstance(value, int) and not isinstance(value, float):
            return 0
        return value


class AddressStructure(BaseModel):
    """
        **Address Structure**

        Address should contain two floats. A latitude and a longitude.

    """
    latitude: float
    longitude: float


class ResponseStructure(BaseModel):
    """
        **Response Structure**

        This is the structure of the response returned to the user. We provide default values for menuUrl and pricepoint.
        If no value is provided then the default values are what is returned in the response.

    """
    microsite: str
    name: str
    logoUrl: HttpUrl
    bookingUrl: HttpUrl
    menuUrl: HttpUrl = None
    cuisineTypes: list
    reviews: ReviewStructure
    address: AddressStructure
    slots: List[SlotStructure]
    pricePoint: int = 0

    @validator('menuUrl', 'logoUrl', pre=True)
    def correct_url_structure(cls, url):
        """
            **Validator on URL**

            Validates the menuURL and logoURL.
            If we do not have a URL ie None, then we return None.
            Otherwise, if we have an HttpURL then we replace the space and return the URL.

        :param url: Menu or Logo
        :return: URL
        :rtype: HttpUrl or None

        """
        if url is None:
            return None
        return url.replace(' ', '%20')

    @validator('pricePoint', pre=True)
    def verify_price_point(cls, price_point):
        """
            **Validator on Price Point**

            If the price point is None we set it to 0.
            We return the value of price point.

        :param price_point:
        :return: pricePoint
        :rtype: int or None

        """
        if price_point is None:
            price_point = 0

        return price_point
