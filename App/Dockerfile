# Set up Tailwind

FROM node:20-slim as styles

COPY ./Tailwind ./
RUN npm install

COPY ./Source ./Source
RUN npx tailwindcss -i ./Styles/tailwind.css -o ./out/tailwind.css

# Build Dotnet App

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
# EXPOSE 8080
EXPOSE 80
# ENV ASPNETCORE_URLS=http://+:8080

WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./*.fsproj ./
RUN dotnet restore

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app
# --no-restore

# Copy build / styles into final location

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
COPY --from=styles ./out/tailwind.css ./wwwroot/css/tailwind.css
ENTRYPOINT ["dotnet", "App.dll"]