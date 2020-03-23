FROM python:3.7-alpine

# Set home as the working directory
WORKDIR /home/

# Copy all the files from the respective directories to the home directory in linux
COPY ./__main__.py .
COPY ./uploader/ /uploader/

# Give executing rights to the main script
RUN chmod +x __main__.py

# Install requirements for Pillow
RUN apk add --no-cache jpeg-dev zlib-dev
RUN apk add --no-cache --virtual .build-deps build-base linux-headers

# Installing the requirements
RUN pip install -r /uploader/requirements.txt

# Copy the crontab 
COPY ./crontab .

# Convert the crontab into linux approritate file
RUN dos2unix /home/crontab

# Execute the crontab with busybox's crond
RUN /usr/bin/crontab /home/crontab

# Start the cronjob
CMD ["/usr/sbin/crond", "-f", "-d" ,"8"]