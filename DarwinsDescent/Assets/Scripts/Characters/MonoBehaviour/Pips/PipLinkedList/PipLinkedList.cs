using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class PipLinkedList : MonoBehaviour
    {
        public PipNode Head;

        /// <summary>
        /// Used to keep track of the amount of nodes in the LL. 
        /// And internally to set the names of created pipnode game objects.
        /// </summary>
        public int Count;

        public PipNode LastTempPip;

        GameObject PipPrefab;

        GameObject Hp_PipPool;

        Queue<GameObject> HpPips;

        public PipLinkedList(
            PipNode head, 
            GameObject pipPrefab, 
            GameObject hp_PipPool, 
            Queue<GameObject> hpPips)
        {
            Head = head;
            Count++;
            PipPrefab = pipPrefab;
            Hp_PipPool = hp_PipPool;
            HpPips = hpPips;
        }

        public void UpdateNodes(PlayerHealth playerHealth)
        {
            PipNode nodeWalker = Head;
            PipNode lastNode = null;
            Count = 0;
            nodeWalker = WalkThroughList(nodeWalker, playerHealth, PipNode.StatusKey.Real, ref lastNode);
            nodeWalker = WalkThroughList(nodeWalker, playerHealth, PipNode.StatusKey.Temp, ref lastNode);
            CheckStateOfTemp(lastNode);
            if (lastNode.PipState == PipNode.StatusKey.Temp)
                LastTempPip = lastNode;

            nodeWalker = WalkThroughList(nodeWalker, playerHealth, PipNode.StatusKey.Lent, ref lastNode);
            nodeWalker = WalkThroughList(nodeWalker, playerHealth, PipNode.StatusKey.Damaged, ref lastNode);
            if (nodeWalker != null)
            {
                if (lastNode.NextNode == LastTempPip)
                    LastTempPip = null;
                lastNode.NextNode = null;
                
                RemoveNodeRecursive(nodeWalker);
            }
        }

        private void CheckStateOfTemp(PipNode LastNode)
        {
            // The Temp pip has changed set the last temp pip to its original size
            if(LastTempPip != null && LastTempPip != LastNode)
            {
                LastTempPip.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public void AddNode(PipNode NewNode)
        {
            Count++;
            //if (NewNode == null)
            //    throw new ArgumentNullException();

            if (Head == null || (Head.PipState > NewNode.PipState))
            {
                NewNode.NextNode = Head;
                Head = NewNode;
                return;
            }

            PipNode prevNode = Head;
            PipNode nextNode = Head.NextNode;

            // walk through the LinkedList until it reaches the end or the new node is smaller than the next.
            // This keeps both the priority and integrity of insertion order.
            while (nextNode != null)
            {
                if (nextNode?.PipState > NewNode.PipState)
                {
                    prevNode.NextNode = NewNode;
                    NewNode.NextNode = nextNode;
                    return;
                }
                prevNode = nextNode;
                nextNode = nextNode.NextNode;
            }

            prevNode.NextNode = PositionNode(prevNode, NewNode);
        }

        private void RemoveNodeRecursive(PipNode pipNode)
        {
            if (pipNode.NextNode != null)
            {
                RemoveNodeRecursive(pipNode.NextNode);
            }

            Destroy(pipNode.gameObject);            
        }

        private PipNode WalkThroughList(PipNode nodeWalker, PlayerHealth playerHealth, PipNode.StatusKey statusKey, ref PipNode lastNode)
        {
            int valueToWalk = 0;
            switch (statusKey)
            {
                case PipNode.StatusKey.Real:
                    valueToWalk = playerHealth.RealHp;
                    break;
                case PipNode.StatusKey.Temp:
                    valueToWalk = playerHealth.TempHp;
                    break;
                case PipNode.StatusKey.Lent:
                    // If for every temp hp, use a lent HPs spot
                    valueToWalk = playerHealth.LentHp - playerHealth.TempHp;
                    break;
                case PipNode.StatusKey.Damaged:
                    valueToWalk = playerHealth.DamagedHealth;
                    break;
            }

            for (int walker = 0; walker < valueToWalk; walker++)
            {
                if (nodeWalker == null)
                {
                    AddNode(new PipNode(statusKey));
                    continue;
                }
                // Count is updated in the add node function so other functionality can add nodes and the count will still be updated properly.
                Count++;
                nodeWalker.PipState = statusKey;
                lastNode = nodeWalker;
                nodeWalker = nodeWalker.NextNode;
            }

            return nodeWalker;
        }

        private PipNode PositionNode(PipNode PreviousNode, PipNode NewNode)
        {
            RectTransform rectTransform = (RectTransform)PreviousNode.gameObject.transform;
            
            // unfortunately, something about setting the Vector 3 in the Instantiate is thrown off, and it ends up adding the Canvas transform to the new items transform
            // instead of the value passed in. Setting the anchored position afterworlds makes it so the Instantiated spawn exactly where they need to.
            RectTransform newrectTransform = null;
            if (NewNode == null)
            {
                NewNode = Instantiate(PipPrefab,
                            new Vector3(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f,
                                        rectTransform.anchoredPosition.y),
                                        rectTransform.transform.rotation,
                                        Hp_PipPool.transform).GetComponent<PipNode>();
                NewNode.gameObject.name = PipPrefab.name + Count.ToString();

                newrectTransform = (RectTransform)NewNode.gameObject.transform;
                newrectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f, rectTransform.anchoredPosition.y);

                return NewNode;
            }

            NewNode.gameObject.transform.position = new Vector3(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f,
                                                                rectTransform.anchoredPosition.y);
            NewNode.gameObject.name = PipPrefab.name + Count.ToString();
            newrectTransform = (RectTransform)NewNode.gameObject.transform;
            newrectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f, rectTransform.anchoredPosition.y);

            return NewNode;
        }
    }
}
