using System;

namespace PinetreeShop.CQRS.Infrastructure
{
    public class IdGenerator
    {
        public static Guid GenerateGuid() { return Guid.NewGuid(); }
    }
}
