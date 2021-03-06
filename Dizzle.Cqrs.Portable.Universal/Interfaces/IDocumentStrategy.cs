﻿//----------------------------------------------------------------------- 
// <copyright file="IDocumentStrategy.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    public interface IDocumentStrategy
    {
        string GetEntityBucket<TEntity>();
        string GetEntityLocation<TEntity>(object key);


        void Serialize<TEntity>(TEntity entity, System.IO.Stream stream);
        TEntity Deserialize<TEntity>(System.IO.Stream stream);
    }

    public interface IDocumentSerializer
    {
        void Serialize<TView>(TView view, System.IO.Stream stream);
        TView Deserialize<TView>(System.IO.Stream stream);
    }
}
