using api.Features.Auth; 

if (args.Length != 1)
{
    Console.WriteLine("Usage: dotnet run -- <password>");
    return;
}

var password = args[0];

// use your real service so format matches Verify()
var svc = new PasswordService();

var hash = svc.Hash(password);
Console.WriteLine(hash);

// to generate hash run this script:
// dotnet run --project server/HashTool -- "MyAdminPassword123!"
