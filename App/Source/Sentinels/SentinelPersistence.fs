namespace CloudSeedApp

open System
open System.Data.Common
open System.Threading.Tasks
open FSharp.Control
open System.Text.Json

open Microsoft.EntityFrameworkCore

open SentinelDomain

module SentinelPersistence = 

    type SentinelDataContext(
        connectionString : string) 
        =
        inherit DbContext()

        [<DefaultValue>]
        val mutable sentinels : DbSet<Sentinel>

        member public this.Sentinels
            with get() = this.sentinels 
            and set s = this.sentinels <- s

        override __.OnConfiguring(optionsBuilder : DbContextOptionsBuilder) = 
            optionsBuilder.UseNpgsql(connectionString)
            |> ignore

        override __.OnModelCreating(modelBuilder : ModelBuilder) = 

            // Sentinels

            modelBuilder.Entity<Sentinel>()
                .ToTable("sentinels")
                |> ignore

            modelBuilder.Entity<Sentinel>()
                .HasKey("id")
                |> ignore

            modelBuilder.Entity<Sentinel>()
                .Property(fun s -> s.id)
                .HasColumnName("id")
                |> ignore

            modelBuilder.Entity<Sentinel>()
                .Property(fun s -> s.data)
                .HasColumnName("data") 
                .HasColumnType("jsonb")
                |> ignore
        
