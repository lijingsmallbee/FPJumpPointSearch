using System;
using System.Collections.Generic;


/**
* @brief Represents an interface to 2D bodies.
**/
namespace TrueSync
{
    public interface IBody
    {
        //
        // Properties
        //
        bool TSDisabled
        {
            get;
            set;
        }

        //
        // Methods
        //
        string Checkum();
    }
}