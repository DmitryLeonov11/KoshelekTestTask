FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./KoshelekTestTask.Client/KoshelekTestTask.Client.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./KoshelekTestTask.Client ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT Docker
ENTRYPOINT ["dotnet", "KoshelekTestTask.Client.dll"]