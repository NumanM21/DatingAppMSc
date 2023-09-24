FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# Future directors will start with /app
WORKDIR /app 
EXPOSE 8080

# copy csproj + restore as distinct layers  (copies packages and restores them)
COPY *.csproj ./ 
RUN dotnet restore 

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Need SDK to do the two steps above, but can use runtime alone to run the app (smaller image)
# build runtime image (copies the published files from the previous step -> SDK large, don't want in our container)
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
# copy from build env, take all contents from app/out  and copy to root of container (.)
COPY --from=build-env /app/out . 
# ENTRYPOINT is the command that will be run when the container starts (looks in API -> Program.cs -. Main())
ENTRYPOINT ["dotnet", "API.dll"]