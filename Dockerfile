# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY WebApi/*.csproj ./WebApi/
COPY Catalogs.Application/*.csproj ./Catalogs.Application/
COPY Catalogs.Domain/*.csproj ./Catalogs.Domain/
COPY Catalogs.Infrastructure/*.csproj ./Catalogs.Infrastructure/
COPY Catalogs.Infrastructure.Mapping/*.csproj ./Catalogs.Infrastructure.Mapping/
COPY Catalogs.Presentation/*.csproj ./Catalogs.Presentation/
COPY Communication.Application/*.csproj ./Communication.Application/
COPY Communication.Domain/*.csproj ./Communication.Domain/
COPY Communication.Infrastructure/*.csproj ./Communication.Infrastructure/
COPY Communication.Infrastructure.Mapping/*.csproj ./Communication.Infrastructure.Mapping/
COPY Communication.Presentation/*.csproj ./Communication.Presentation/
COPY Core/*.csproj ./Core/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Shoppings.Application/*.csproj ./Shoppings.Application/
COPY Shoppings.Domain/*.csproj ./Shoppings.Domain/
COPY Shoppings.Infrastructure/*.csproj ./Shoppings.Infrastructure/
COPY Shoppings.Infrastructure.Mapping/*.csproj ./Shoppings.Infrastructure.Mapping/
COPY Shoppings.Presentation/*.csproj ./Shoppings.Presentation/
COPY User.Infrastructure/*.csproj ./User.Infrastructure/
COPY Users.Application/*.csproj ./Users.Application/
COPY Users.Domain/*.csproj ./Users.Domain/
COPY Users.Infrastructure.Mapping/*.csproj ./Users.Infrastructure.Mapping/
COPY Users.Presentation/*.csproj ./Users.Presentation/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Publish the app
RUN dotnet publish WebApi/WebApi.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll"] 