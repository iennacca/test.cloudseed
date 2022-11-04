FROM node:lts-slim as build

WORKDIR /app

COPY package*.json ./
RUN rm -rf node_modules
RUN rm -rf build
COPY . .
RUN npm install
RUN npm run build

FROM node:lts-slim as run

WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT [ "npm", "run", "start" ]