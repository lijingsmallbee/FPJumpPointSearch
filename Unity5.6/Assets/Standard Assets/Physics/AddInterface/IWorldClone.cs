using System;
using System.Collections.Generic;


namespace TrueSync
{
    public interface IWorldClone
    {
        //
        // Properties
        //
        string checksum
        {
            get;
        }

        //
        // Methods
        //
        void Clone(IWorld iWorld);

        void Clone(IWorld iWorld, bool doChecksum);

        void Restore(IWorld iWorld);
    }
}