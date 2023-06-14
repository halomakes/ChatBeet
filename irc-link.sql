create or replace procedure merge_irc_nick (
	user_irc_nick varchar(20),
	user_discord_id numeric(20)
)
language plpgsql
as 
$$
declare
user_mention varchar(30) := concat('<@', cast(user_discord_id as varchar), '>');
	old_id uuid;
	new_id uuid;
begin
select u.id into old_id from core.users u where lower(u.irc_nick) = lower(user_irc_nick);
select u.id into new_id from core.users u where u.discord_id  = user_discord_id;
raise notice 'Value of user_irc_nick: %', user_irc_nick;
	raise notice 'Value of user_discord_id: %', user_discord_id;
	raise notice 'Value of user_mention: %', user_mention;
	raise notice 'Value of old_id: %', old_id;
	raise notice 'Value of new_id: %', new_id;

	-- overwrite any old preferences
delete from core.user_preferences where user_id = old_id and preference in (select preference from core.user_preferences where user_id = new_id);

update core.user_preferences set user_id = new_id where user_id = old_id;
update booru.blacklisted_tags set user_id = new_id where user_id = old_id;
update booru.tag_history set user_id = new_id where user_id = old_id;
update interactions.definitions set created_by = new_id where created_by = old_id;
update interactions.high_ground set user_id  = new_id where user_id = old_id;
update interactions.karma set "key" = user_mention where lower("key") = lower(user_irc_nick);
update interactions.quotes set saved_by_id = new_id where saved_by_id = old_id;
update interactions.quote_message set author_id = new_id where author_id = old_id;
update interactions.suspicion_report set reporter_id = new_id where reporter_id = old_id;
update interactions.suspicion_report set suspect_id = new_id where suspect_id = old_id;
update stats.events e set triggering_user_id = new_id where triggering_user_id = old_id;
update stats.events e set targeted_user_id = new_id where targeted_user_id  = old_id;

update core.users set irc_nick = user_irc_nick where id = new_id;
delete from core.users where id = old_id;

end $$;