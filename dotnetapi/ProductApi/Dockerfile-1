FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /
COPY ["dotnetapi/TestLib/TestLib.csproj", "dotnetapi/TestLib/"]
COPY ["dotnetapi/ProductApi/ProductApi.csproj", "dotnetapi/ProductApi/"]
RUN dotnet restore "dotnetapi/ProductApi/ProductApi.csproj"
COPY . .
WORKDIR "/dotnetapi/ProductApi"
RUN dotnet build "ProductApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductApi.dll"]
