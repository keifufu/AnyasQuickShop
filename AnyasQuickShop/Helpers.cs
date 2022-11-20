namespace AnyasQuickShop
{
    internal class Helpers
    {
        public void UnmountAllBodyParts(CarLoader carLoader)
        {
            foreach (CarPart part in carLoader.carParts)
            {
                if (part.name != "body" && part.name != "details")
                    carLoader.TakeOffCarPart(part.name);
            }
        }
    }
}
