using S8l.CustomPlatformSharedInterfaces.Runtime;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public abstract class BasePlatform : IPlatform
    {
        public virtual bool IsThisMyPlatform()
        {
            throw new System.NotImplementedException();
        }
        

        public virtual int Generality { get; } = 0;
        
        public override string ToString()
        {
            return this.GetType().Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            // Same reference matches
            //if (System.Object.ReferenceEquals(this, obj))
            //{
            //    return true;
            //}

            // Basically same type is enough for a match
            if (this.GetType() == obj.GetType())
            {
                return true;
            }

            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}