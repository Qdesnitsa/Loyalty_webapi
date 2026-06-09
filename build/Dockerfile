FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH

WORKDIR /src

COPY Loyalty/Loyalty/Loyalty.Api/*.csproj Loyalty/Loyalty/Loyalty.Api/
COPY Loyalty/Loyalty/Loyalty.Application/*.csproj Loyalty/Loyalty/Loyalty.Application/
COPY Loyalty/Loyalty/Loyalty.Infrastructure/*.csproj Loyalty/Loyalty/Loyalty.Infrastructure/
COPY Loyalty/Loyalty/Loyalty.Domain/*.csproj Loyalty/Loyalty/Loyalty.Domain/
COPY WebMoney/WebMoney/WebMoney.Loyalty.Events/*.csproj WebMoney/WebMoney/WebMoney.Loyalty.Events/

RUN dotnet restore "Loyalty/Loyalty/Loyalty.Api/Loyalty.Api.csproj" -a $TARGETARCH

COPY Loyalty/Loyalty/ Loyalty/Loyalty/
COPY WebMoney/WebMoney/WebMoney.Loyalty.Events/ WebMoney/WebMoney/WebMoney.Loyalty.Events/

WORKDIR /src/Loyalty/Loyalty/Loyalty.Api
RUN dotnet publish --no-restore -c Release -o /app/publish -a $TARGETARCH

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

ENV ASPNETCORE_URLS=http://*:8080

WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "Loyalty.Api.dll"]
