# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0.100-alpine3.14 AS build
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

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "App.dll"]