FROM redis
LABEL maintainer="rodrigodotnet@gmail.com"
# Define mountable directories.
VOLUME ["/data"]
# Define working directory.
WORKDIR /data
EXPOSE 6379
HEALTHCHECK CMD ["docker-healthcheck"]