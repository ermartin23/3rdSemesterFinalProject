using api.Features.Auth; 

if (args.Length != 1)
{
    Console.WriteLine("Usage: dotnet run -- <password>");
    return;
}

var password = args[0];

var svc = new PasswordService();

var hash = svc.Hash(password);
Console.WriteLine(hash);