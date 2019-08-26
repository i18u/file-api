FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY ./src/Files.Web/ ./Files.Web/

RUN dotnet publish ./Files.Web/Files.Web.csproj -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

COPY --from=build-env /app/Files.Web/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "Files.Web.dll"]
