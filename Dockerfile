FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /src

COPY *.slnx ./
COPY Directory.Packages.props ./

COPY src/RestaurantReservation.Domain/*.csproj ./src/RestaurantReservation.Domain/
COPY src/RestaurantReservation.Application/*.csproj ./src/RestaurantReservation.Application/
COPY src/RestaurantReservation.Infrastructure/*.csproj ./src/RestaurantReservation.Infrastructure/
COPY src/RestaurantReservation.Api/*.csproj ./src/RestaurantReservation.Api/

RUN dotnet restore src/RestaurantReservation.Api/RestaurantReservation.Api.csproj

COPY . .

RUN dotnet publish src/RestaurantReservation.Api/RestaurantReservation.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "RestaurantReservation.Api.dll"]