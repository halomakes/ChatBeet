﻿// <auto-generated />
using System;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatBeet.Migrations
{
    [DbContext(typeof(CbDbContext))]
    [Migration("20230223005658_StatEventTableName")]
    partial class StatEventTableName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatBeet.Data.Entities.BlacklistedTag", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<string>("Tag")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("tag");

                    b.HasKey("UserId", "Tag")
                        .HasName("pk_blacklisted_tags");

                    b.ToTable("blacklisted_tags", "booru");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Definition", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("key");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(3000)
                        .HasColumnType("character varying(3000)")
                        .HasColumnName("value");

                    b.HasKey("Key", "GuildId")
                        .HasName("pk_definitions");

                    b.HasIndex("CreatedBy")
                        .HasDatabaseName("ix_definitions_created_by");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_definitions_guild_id");

                    b.ToTable("definitions", "interactions");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Guild", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<DateTime>("AddedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("added_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("AddedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("added_by");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("label");

                    b.HasKey("Id")
                        .HasName("pk_guilds");

                    b.HasIndex("AddedBy")
                        .HasDatabaseName("ix_guilds_added_by");

                    b.ToTable("guilds", "core");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.HighGround", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("GuildId")
                        .HasName("pk_high_ground");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_high_ground_user_id");

                    b.ToTable("high_ground", "interactions");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.KarmaVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("key");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<Guid>("VoterId")
                        .HasColumnType("uuid")
                        .HasColumnName("voter_id");

                    b.HasKey("Id")
                        .HasName("pk_karma");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_karma_guild_id");

                    b.HasIndex("VoterId")
                        .HasDatabaseName("ix_karma_voter_id");

                    b.ToTable("karma", "interactions");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Keyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("label");

                    b.Property<string>("Regex")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("regex");

                    b.Property<int>("SortOrder")
                        .HasColumnType("integer")
                        .HasColumnName("sort_order");

                    b.HasKey("Id")
                        .HasName("pk_keywords");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_keywords_guild_id");

                    b.ToTable("keywords", "stats");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.KeywordHit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<Guid>("KeywordId")
                        .HasColumnType("uuid")
                        .HasColumnName("keyword_id");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_keyword_hits");

                    b.HasIndex("KeywordId")
                        .HasDatabaseName("ix_keyword_hits_keyword_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_keyword_hits_user_id");

                    b.ToTable("keyword_hits", "stats");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.ProgressSpan", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("key");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("AfterRangeMessage")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("after_range_message");

                    b.Property<string>("BeforeRangeMessage")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("before_range_message");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<string>("Template")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("template");

                    b.HasKey("Key", "GuildId")
                        .HasName("pk_progress_spans");

                    b.ToTable("progress_spans", "interactions");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.StatEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("OccurredAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occurred_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("SampleText")
                        .HasColumnType("text")
                        .HasColumnName("sample_text");

                    b.Property<Guid?>("TargetedUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("targeted_user_id");

                    b.Property<Guid>("TriggeringUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("triggering_user_id");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_events_guild_id");

                    b.HasIndex("TargetedUserId")
                        .HasDatabaseName("ix_events_targeted_user_id");

                    b.HasIndex("TriggeringUserId")
                        .HasDatabaseName("ix_events_triggering_user_id");

                    b.ToTable("events", "stats");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.SuspicionReport", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<Guid>("ReporterId")
                        .HasColumnType("uuid")
                        .HasColumnName("reporter_id");

                    b.Property<Guid>("SuspectId")
                        .HasColumnType("uuid")
                        .HasColumnName("suspect_id");

                    b.HasKey("Id", "CreatedAt")
                        .HasName("pk_suspicion_report");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_suspicion_report_guild_id");

                    b.HasIndex("ReporterId")
                        .HasDatabaseName("ix_suspicion_report_reporter_id");

                    b.HasIndex("SuspectId")
                        .HasDatabaseName("ix_suspicion_report_suspect_id");

                    b.ToTable("suspicion_report", "interactions");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.TagHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("tag");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_tag_history");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_tag_history_user_id");

                    b.ToTable("tag_history", "booru");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.TopTag", b =>
                {
                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tag");

                    b.Property<int>("Total")
                        .HasColumnType("integer")
                        .HasColumnName("total");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_top_tags_user_id");

                    b.ToTable("top_tags", (string)null);
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("current_timestamp");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", "core");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.UserPreferenceSetting", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("Preference")
                        .HasColumnType("integer")
                        .HasColumnName("preference");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("value");

                    b.HasKey("UserId", "Preference")
                        .HasName("pk_user_preferences");

                    b.ToTable("user_preferences", "core");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.BlacklistedTag", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_blacklisted_tags_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Definition", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_definitions_users_author_id");

                    b.HasOne("ChatBeet.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_definitions_guilds_guild_id");

                    b.Navigation("Author");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Guild", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_guilds_users_added_by_user_id");

                    b.Navigation("AddedByUser");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.HighGround", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_high_ground_guilds_guild_id");

                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_high_ground_users_user_id");

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.KarmaVote", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_karma_guilds_guild_id");

                    b.HasOne("ChatBeet.Data.Entities.User", "Voter")
                        .WithMany()
                        .HasForeignKey("VoterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_karma_users_voter_id");

                    b.Navigation("Guild");

                    b.Navigation("Voter");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Keyword", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_keywords_guilds_guild_id");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.KeywordHit", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Keyword", null)
                        .WithMany("Hits")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_keyword_hits_keywords_keyword_id");

                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_keyword_hits_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.StatEvent", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Guild", null)
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_events_guilds_guild_id");

                    b.HasOne("ChatBeet.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("TargetedUserId")
                        .HasConstraintName("fk_events_users_user_id");

                    b.HasOne("ChatBeet.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("TriggeringUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_events_users_user_id1");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.SuspicionReport", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_suspicion_report_guilds_guild_id");

                    b.HasOne("ChatBeet.Data.Entities.User", "Reporter")
                        .WithMany()
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_suspicion_report_users_reporter_id");

                    b.HasOne("ChatBeet.Data.Entities.User", "Suspect")
                        .WithMany()
                        .HasForeignKey("SuspectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_suspicion_report_users_suspect_id");

                    b.Navigation("Guild");

                    b.Navigation("Reporter");

                    b.Navigation("Suspect");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.TagHistory", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_tag_history_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.TopTag", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_top_tags_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.User", b =>
                {
                    b.OwnsOne("ChatBeet.Data.Entities.DiscordIdentity", "Discord", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Discriminator")
                                .HasMaxLength(10)
                                .HasColumnType("character varying(10)")
                                .HasColumnName("discord_discriminator");

                            b1.Property<decimal?>("Id")
                                .HasColumnType("numeric(20,0)")
                                .HasColumnName("discord_id");

                            b1.Property<string>("Name")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("discord_name");

                            b1.HasKey("UserId");

                            b1.ToTable("users", "core");

                            b1.WithOwner()
                                .HasForeignKey("UserId")
                                .HasConstraintName("fk_users_users_id");
                        });

                    b.OwnsOne("ChatBeet.Data.Entities.IrcIdentity", "Irc", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Nick")
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("irc_nick");

                            b1.HasKey("UserId");

                            b1.ToTable("users", "core");

                            b1.WithOwner()
                                .HasForeignKey("UserId")
                                .HasConstraintName("fk_users_users_id");
                        });

                    b.Navigation("Discord");

                    b.Navigation("Irc");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.UserPreferenceSetting", b =>
                {
                    b.HasOne("ChatBeet.Data.Entities.User", "User")
                        .WithMany("Preferences")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_preferences_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.Keyword", b =>
                {
                    b.Navigation("Hits");
                });

            modelBuilder.Entity("ChatBeet.Data.Entities.User", b =>
                {
                    b.Navigation("Preferences");
                });
#pragma warning restore 612, 618
        }
    }
}
