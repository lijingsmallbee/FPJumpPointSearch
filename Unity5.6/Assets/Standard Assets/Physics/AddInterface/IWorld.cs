using System;
using System.Collections.Generic;


    /**
    * @brief Represents an interface to 2D bodies.
    **/
    namespace TrueSync
    {
        public interface IWorld
        {
            //
            // Methods
            //
            List<IBody> Bodies();
        }
    }