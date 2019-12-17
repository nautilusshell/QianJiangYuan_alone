﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Utils.Json;

namespace WindowsAuth.models
{
    /// <summary>
    /// Class to communicate with WinBind server. 
    /// </summary>
    [DataContract]
    public class UserID
    {
        [DataMember(Name = "uid")]
        public string uid { get; set; }

        [DataMember(Name = "gid")]
        public string gid { get; set; }

        [DataMember(Name = "groups")]
        public List<string> groups { get; set; }

        public string isAdmin { get; set; }
        public string isAuthorized { get; set; }
    }

    /// <summary>
    /// User entry class in Database
    /// </summary>
    public partial class UserEntry
    {
        [Key]
        public string Email { get; set; }
        // Username to be used. 
        public string Alias { get; set; }
        public string Password { get; set; }
        public string uid { get; set; }
        public string gid { get; set; }
        public string isAdmin { get; set; }
        public string isAuthorized { get; set; }
        /// <summary>
        /// The following entry is reserved for future usage. 
        /// </summary>
        public string Config { get; set; }
        public string ConfigSecret { get; set; }
        public string Other { get; set; }
        public UserEntry()
        {
        }

        public UserEntry(UserID userID, string email, string alias, string password)
        {
            Email = email;
            Alias = alias;
            Password = password;
            uid = userID.uid;
            gid = userID.gid;
            isAdmin = userID.isAdmin;
            isAuthorized = userID.isAuthorized;
        }
        public JObject toJObject()
        {
            var ret = new JObject();
            ret["Email"] = Email;
            if ( !String.IsNullOrEmpty(Alias))
            { 
                ret["Alias"] = Alias;
            }
            if (!String.IsNullOrEmpty(Password))
            {
                ret["Password"] = Password;
            }
            if (!String.IsNullOrEmpty(uid))
            {
                ret["uid"] = uid;
            }
            if (!String.IsNullOrEmpty(gid))
            {
                ret["gid"] = gid;
            }
            ret["isAdmin"] = isAdmin.Equals("true");
            ret["isAuthorized"] = isAuthorized.Equals("true");

            return ret; 
        }
        static UserEntry Parse( JObject obj)
        {
            var email = JsonUtils.GetString("Email", obj);
            if ( String.IsNullOrEmpty(email))
            {
                return null; 
            }
            var entry = new UserEntry();
            entry.Email = email; 
            var Alias = JsonUtils.GetString("Alias", obj);
            if (!String.IsNullOrEmpty(Alias))
            {
                entry.Alias = Alias; 
            }
            var Password = JsonUtils.GetString("Password", obj);
            if (!String.IsNullOrEmpty(Password))
            {
                entry.Password = Password;
            }
            var uid = JsonUtils.GetString("uid", obj);
            if (!String.IsNullOrEmpty(uid))
            {
                entry.uid = uid;
            }
            var gid = JsonUtils.GetString("gid", obj);
            if (!String.IsNullOrEmpty(gid))
            {
                entry.gid = gid;
            }
            var isAdmin = JsonUtils.GetBool("isAdmin", obj);
            entry.isAdmin = isAdmin ? "true" : "false";
            var isAuthorized = JsonUtils.GetBool("isAuthorized", obj);
            entry.isAuthorized = isAuthorized ? "true" : "false";
            return entry;
        }
    }

    /// <summary>
    /// Template entry class in Database
    /// </summary>
    public partial class TemplateEntry
    {
        [Key]
        public string Template { get; set; }
        public string Username { get; set; }
        public string Json { get; set; }
        public string Type { get; set; }

        public TemplateEntry()
        {
        }

        public TemplateEntry(string template, string username, string json, string type)
        {
            Template = template;
            Username = username;
            Json = json;
            Type = type;
        }
    }

    public partial class ClusterContext : DbContext
    {
        /*
        private string _connectionString; 
        public UserContext(string connectionString )
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            
        } */
        public ClusterContext(DbContextOptions<ClusterContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Asp Net Identity Tables
            builder.Entity<UserEntry>().ToTable("User");
            builder.Entity<UserEntry>().HasKey(c => c.Email);
            builder.Entity<UserEntry>().Property(u => u.Email).HasMaxLength(128);
            builder.Entity<UserEntry>().Property(u => u.Alias).HasMaxLength(128);
            builder.Entity<UserEntry>().Property(u => u.Password).HasMaxLength(64);
            builder.Entity<UserEntry>().Property(u => u.uid).HasMaxLength(64);
            builder.Entity<UserEntry>().Property(u => u.gid).HasMaxLength(64);
            builder.Entity<UserEntry>().Property(u => u.isAdmin).HasMaxLength(10);
            builder.Entity<UserEntry>().Property(u => u.isAuthorized).HasMaxLength(10);

            // Configure Asp Net Identity Tables
            builder.Entity<TemplateEntry>().ToTable("Template");
            builder.Entity<TemplateEntry>().HasKey(c => c.Template);
            builder.Entity<TemplateEntry>().Property(u => u.Template).HasMaxLength(128);
            builder.Entity<TemplateEntry>().Property(u => u.Username).HasMaxLength(128);
            builder.Entity<TemplateEntry>().Property(u => u.Json);
            builder.Entity<TemplateEntry>().Property(u => u.Type).HasMaxLength(10);
        }

        public virtual DbSet<UserEntry> User { get; set; }
        public virtual DbSet<TemplateEntry> Template { get; set; }
    }

    public class ClusterSelectViewModel
    {
        public string CurrentCluster { get; set; }
        public List<SelectListItem> ClustersList { get; set; }
    }
}
