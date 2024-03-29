﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApiApplication.Models
{
    public class WebApiDBContext : DbContext
    {
        public WebApiDBContext() : base("name=DefaultConnection")
        {

        }

        public DbSet<ECPay> ECPays { get; set; }
        public DbSet<ECPay_LogisticsStatus> ECPay_LogisticsStatus { get; set; }
        public DbSet<ECPay_TempTradeResult> ECPay_TempTradeResults { get; set; }
        public DbSet<User> Users { get; set; }
    }
}