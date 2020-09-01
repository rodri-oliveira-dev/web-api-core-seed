FROM datalust/seq:latest
LABEL maintainer="rodrigodotnet@gmail.com"
ENV ACCEPT_EULA=Y
VOLUME /path/to/seq/data:/data
EXPOSE 5341:5341
EXPOSE 1780:80
