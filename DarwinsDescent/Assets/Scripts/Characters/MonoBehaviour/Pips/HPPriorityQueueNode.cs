﻿using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent
{
    public class HPPriorityQueueNode : FastPriorityQueueNode
    {
        public HPPipModel PipModel { get; set; }

        public HPPriorityQueueNode(HPPipModel pipModel)
        {
            PipModel = pipModel;
        }
    }
}
