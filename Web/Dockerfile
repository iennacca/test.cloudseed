FROM node:12

WORKDIR /usr/src/app

COPY package*.json ./
RUN rm -rf node_modules
RUN npm install
COPY . .
RUN npm run build

EXPOSE 8080
# RUN npm run dev
ENTRYPOINT [ "npm", "run", "start" ]