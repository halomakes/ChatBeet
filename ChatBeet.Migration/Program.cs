using ChatBeet.Migration;

var serverId = ulong.Parse(args[0]);
var connectionString = Environment.GetEnvironmentVariable("ChatBeet:Postgres");
var migrator = new Migrator(serverId, connectionString!);
await migrator.Migrate();