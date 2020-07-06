using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class GroupPath : ItemPath
    {
        readonly List<IItemPath> itemPaths = new List<IItemPath>(10);

        public void AddPath(IItemPath itemPath)
        {
            this.itemPaths.Add(itemPath);

            this.MaximumFrame += itemPath.MaximumFrame;
        }

        public override void GetPosition(int currentFrame, out int offsetX, out int offsetY)
        {
            if(itemPaths.Count == 0)
            {
                offsetX = 0;
                offsetY = 0;
                return;
            }

            var maxFrame = 0;
            var oldMaxFrame = 0;
            IItemPath selectedItemPath = null;

            int x = 0;
            int y = 0;

            for(int i = 0; i < itemPaths.Count; i++)
            {
                selectedItemPath = itemPaths[i];

                oldMaxFrame = maxFrame;
                maxFrame += selectedItemPath.MaximumFrame;

                if (currentFrame < maxFrame)
                {
                    break;
                }

                selectedItemPath.GetPosition(selectedItemPath.MaximumFrame, out var lastOffsetX, out var lastOffsetY);

                x += lastOffsetX;
                y += lastOffsetY;
            }

            // currentFrame est desormais relatif à l'itemPath
            currentFrame -= oldMaxFrame;

            selectedItemPath.GetPosition(currentFrame, out var currentOffsetX, out var currentOffsetY);

            offsetX = x + currentOffsetX;
            offsetY = y + currentOffsetY;
        }
    }
}
