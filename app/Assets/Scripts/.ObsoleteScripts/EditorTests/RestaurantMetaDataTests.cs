using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RestaurantMetaDataTests
    {
        RestaurantMetadata metadata;
        Dictionary<string, string> testData;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/CustomerDemo.unity");
            metadata = GameObject.FindObjectOfType<RestaurantMetadata>();
        }

        [Test]
        public void RestaurantMetadataIsNotNullAfterCall()
        {
            Assert.IsNotNull(metadata.GetRestaurantMetadata("kfc"));
        }

        [Test]
        public void RestaurantMetadataIsNotEmpty()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").Count > 0);
        }

        [Test]
        public void RestaurantMetadataContainsName()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("name"));
        }

        [Test]
        public void RestaurantMetadataContainsCuisineType()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("cuisine"));
        }

        [Test]
        public void RestaurantMetadataContainsSeats()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("seats"));
        }

        [Test]
        public void RestaurantMetadataContainsReview()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("review"));
        }

        [Test]
        public void RestaurantMetadataContainsPrice()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("price"));
        }

        [Test]
        public void RestaurantMetadataContainsImage()
        {
            Assert.IsTrue(metadata.GetRestaurantMetadata("kfc").ContainsKey("image"));
        }


        [Test]
        public void RestaurantMetadataReturnsCorrectName()
        {
            string name = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("name", out name);
            StringAssert.Contains("kfc", name);
        }

        [Test]
        public void RestaurantMetadataReturnsCorrectCuisineType()
        {
            string cuisine = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("cuisine", out cuisine);
            StringAssert.Contains("placeholder-cuisine", cuisine);
        }

        [Test]
        public void RestaurantMetadataReturnsCorrectSeatAvailability()
        {
            string seats = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("seats", out seats);
            StringAssert.Contains("placeholder-seats", seats);
        }

        [Test]
        public void RestaurantMetadataReturnsCorrectReview()
        {
            string review = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("review", out review);
            StringAssert.Contains("placeholder-review", review);
        }

        [Test]
        public void RestaurantMetadataReturnsCorrectPrice()
        {
            string price = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("price", out price);
            StringAssert.Contains("placeholder-price", price);
        }

        [Test]
        public void RestaurantMetadataReturnsCorrectImage()
        {
            string image = "";
            metadata.GetRestaurantMetadata("kfc").TryGetValue("image", out image);
            StringAssert.Contains("placeholder-image", image);
        }


        [Test]
        public void RestaurantMetadataFieldReferencesAreNotNull()
        {
            Assert.IsNotNull(metadata.restaurantNameField, "NULL reference to restaurant name text field");
            Assert.IsNotNull(metadata.cuisineTypeField, "NULL reference to cuisine text field");
            Assert.IsNotNull(metadata.seatAvailabilityField, "NULL reference to seat availability text field");
            Assert.IsNotNull(metadata.reviewField, "NULL reference to review text field");
            Assert.IsNotNull(metadata.pricePointField, "NULL reference to price text field");
            Assert.IsNotNull(metadata.logoImageField, "NULL reference to logo image field");
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }
    }
}
