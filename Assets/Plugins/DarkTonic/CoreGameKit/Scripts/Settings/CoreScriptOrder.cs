/*! \cond PRIVATE */
using System;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    public class CoreScriptOrder : Attribute {
        public int Order;

        public CoreScriptOrder(int order) {
            Order = order;
        }
    }
}
/*! \endcond */