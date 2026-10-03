using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        // JoinDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class
    }
}
