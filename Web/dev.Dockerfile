FROM node:20-slim

WORKDIR /app

COPY package*.json ./
RUN rm -rf node_modules
RUN npm install
COPY . .

# We explicitly run build to catch any TS bugs we may have introduced in dev
# Currently nextjs run dev does _not_ catch bugs - https://github.com/vercel/next.js/issues/14997
# This introduces some major slowdowns, but better to catch early than at production
RUN npm run build

EXPOSE 8080
EXPOSE 24678
# RUN npm run dev
ENTRYPOINT [ "npm", "run", "dev" ]