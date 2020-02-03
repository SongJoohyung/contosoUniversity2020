using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models
{
    //mwilliams: Part 2: Create the data models
    //1. Create the person abstract class (inheritance)
    public abstract class Person
    {
        //The ID property wil become the PK column of the datatbase table
        //by default, the Entity Framework (EF) interprets a property name "ID" or
        //"ClassnameID" as the Primary Key (PK)
        public int ID { get; set; }

        //String types will become nvarchar(max) by default, we will override that by
        //using StringLength

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(65, ErrorMessage = "Last name cannot be longer than 65 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(85, ErrorMessage = "Email cannot be longer than 85 characters")]
        public string Email { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Address cannot be longer than 150 characters")]
        public string Address { get; set; }

        [Required]
        [StringLength(60)]
        public string City { get; set; }

        [Required]
        [StringLength(2)]
        [Column(TypeName = "nchar(2)")] //nchar(2)
        public string Province { get; set; }

        [Required]
        [StringLength(7)]
        [Column(TypeName = "nchar(7)")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
            [Display(Name = "Name")]
            public string FullNameAlt
        {
            get
            {
                return FirstName + ", " + LastName;
            }
        }
        [Display(Name = "Name")]
        public string IdFullName
        {
            get
            {
                return "(" + ID + ")" + FullNameAlt;
            }
        }

        public string FullAddress
        {
            get
            {
                return Address + ", " +
                    City + ", " +
                    Province + " " +
                    PostalCode;
            }
        }
    }

}