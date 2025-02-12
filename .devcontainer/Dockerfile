ARG NODE_IMG="18-slim"
ARG VARIANT="8.0"
FROM node:${NODE_IMG} as node
FROM mcr.microsoft.com/dotnet/sdk:${VARIANT}

# Copy node runtime from multi-stage build
# https://github.com/BretFisher/nodejs-rocks-in-docker/blob/45354b2745f7accddea46a5fa7ef820c9388b1a5/dockerfiles/ubuntu-copy.Dockerfile#L17-L21
COPY --from=node /usr/local/include/ /usr/local/include/
COPY --from=node /usr/local/lib/ /usr/local/lib/
COPY --from=node /usr/local/bin/ /usr/local/bin/

# Install basic development tools
RUN apt update && apt install -y less man-db sudo

# Ensure default `node` user has access to `sudo`
ARG USERNAME=node
RUN echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME

# Set `DEVCONTAINER` environment variable to help with orientation
ENV DEVCONTAINER=true
