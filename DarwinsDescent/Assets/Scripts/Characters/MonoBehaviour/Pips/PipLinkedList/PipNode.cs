using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DarwinsDescent
{
    public class PipNode : MonoBehaviour
    {
        public Image PipDisplayImage;

        public StatusKey PipState;

        public PipNode NextNode;

        //public PipNode PreviousNode { get; set; }

        public PipNode(StatusKey pipState, PipNode nextNode = null)//, PipNode previousNode = null)
        {
            PipState = pipState;
            NextNode = nextNode;
            //PreviousNode = previousNode;
        }

        public enum StatusKey
        {
            Real,
            Temp,
            Lent,
            Damaged
        }
    }
}
