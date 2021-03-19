using GlycanQuant.Model.Util;
using GlycanQuant.Model;
using GlycanQuant.Model.NGlycans;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FormulaTest()
        {
            //    Dictionary<Element, int> compounds; ;
            //    // GlcNAc C8H15NO6, Man/Gal C6H12O6, Fuc C6H12O5,  NeuAc C11H19NO9, NeuGc C11H19NO10
            //    compounds = new Dictionary<Element, int>()
            //    {
            //        //{new C(), 8}, {new H(), 15}, {new N(), 1}, {new O(), 6}
            //        //{new C(), 6}, {new H(), 12}, {new O(), 6}
            //        //{new C(), 6}, {new H(), 12}, {new O(), 5}
            //        //{new C(), 11}, {new H(), 19}, {new N(), 1}, {new O(), 9}
            //        {new C(), 11}, {new H(), 19}, {new N(), 1}, {new O(), 10}
            //    };

            //    double mass = 0;
            //    foreach(Element key in compounds.Keys)
            //    {
            //        int num = compounds[key];

            //        // water H2O
            //        if (key.Atomic == 1)
            //        {
            //            num -= 2;
            //        }
            //        else if (key.Atomic == 16)
            //        {
            //            num -= 1;
            //        }

            //        //// methyl replace OCH3 -> OH (3), HexNAc, Hex
            //        //if (key.Atomic == 1)
            //        //{
            //        //    num += 6;
            //        //}
            //        //else if (key.Atomic == 12)
            //        //{
            //        //    num += 3;
            //        //}

            //        // methyl replace OCH3 -> OH (2), Fuc
            //        //if (key.Atomic == 1)
            //        //{
            //        //    num += 4;
            //        //}
            //        //else if (key.Atomic == 12)
            //        //{
            //        //    num += 2;
            //        //}

            //        // methyl replace OCH3 -> OH (5), NeuAc
            //        //if (key.Atomic == 1)
            //        //{
            //        //    num += 10;
            //        //}
            //        //else if (key.Atomic == 12)
            //        //{
            //        //    num += 5;
            //        //}

            //        // methyl replace OCH3 -> OH (6), NeuAc
            //        if (key.Atomic == 1)
            //        {
            //            num += 12;
            //        }
            //        else if (key.Atomic == 12)
            //        {
            //            num += 6;
            //        }


            //        mass += key.MonoMass * num;
            //    }

            double mass = 0;
            Dictionary<Element, int> compose = NMonosaccharideCreator.Get.SubCompositions(Monosaccharide.NeuGc, true);
            foreach (Element key in compose.Keys)
            {
                mass += key.MonoMass * compose[key];
            }

            Console.WriteLine(mass);

            ////GlcNAc
            //Assert.AreEqual(mass, 245.1263, 0.001);
            ////kHex
            //Assert.AreEqual(mass, 204.0998, 0.001);
            ////kFuc
            //Assert.AreEqual(mass, 174.0892, 0.001);
            ////NeuAc
            //Assert.AreEqual(361.1737, mass, 0.001);
            //NeuGc
            Assert.AreEqual(mass, 391.1842, 0.001);
        }
    }
}