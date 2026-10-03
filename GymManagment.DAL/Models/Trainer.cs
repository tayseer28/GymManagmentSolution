using GymManagment.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }
        // HireDate is the CreatedAt of the BaseEntity so we will handle this in the configuration class
    }
}
