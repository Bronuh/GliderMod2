dotnet run --project ../ZZCakeBuild/CakeBuild.csproj -t="SetupTestEnvironment" --testEnvPath="../TestEnv" -- $args
exit $LASTEXITCODE;