﻿// <auto-generated />
using Hangman;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseInitCrap.Migrations
{
    [DbContext(typeof(ScoreContext))]
    partial class ScoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("Hangman.Score", b =>
                {
                    b.Property<int>("ScoreId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Value");

                    b.Property<string>("Word");

                    b.HasKey("ScoreId");

                    b.ToTable("Scores");
                });
#pragma warning restore 612, 618
        }
    }
}
