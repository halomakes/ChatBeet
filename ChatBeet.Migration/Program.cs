using ChatBeet.Migration;

var serverId = ulong.Parse(args[0]);
var migrator = new Migrator(serverId);
await migrator.Migrate();