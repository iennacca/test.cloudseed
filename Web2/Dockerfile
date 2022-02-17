FROM node:16

WORKDIR /app

COPY package*.json ./
RUN rm -rf node_modules
RUN rm -rf build
COPY . .
RUN npm install
RUN npm run build

EXPOSE 8080
# RUN npm run dev
# ENTRYPOINT [ "node", "./index.js" ]
ENTRYPOINT [ "npm", "run", "start" ]