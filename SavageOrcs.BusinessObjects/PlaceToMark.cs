﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class PlaceToMark
    {
        public Guid Id { get; set; }

        public Guid MarkId { get; set; }
        public virtual Mark Mark { get; set; }

        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; }
    }


    public class PlaceToCluster
    {
        public Guid Id { get; set; }

        public Guid ClusterId { get; set; }
        public virtual Cluster Cluster { get; set; }

        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; }
    }
    
}
