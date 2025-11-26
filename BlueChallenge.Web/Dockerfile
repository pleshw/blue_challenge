# syntax=docker/dockerfile:1.7

ARG NODE_VERSION=20.18.1

FROM node:${NODE_VERSION}-alpine AS build
WORKDIR /app

COPY BlueChallenge.Web/package*.json ./
RUN npm ci

COPY BlueChallenge.Web/ .
RUN npm run build

FROM nginx:1.27-alpine AS final
WORKDIR /usr/share/nginx/html
COPY BlueChallenge.Web/nginx/default.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist .
EXPOSE 80
