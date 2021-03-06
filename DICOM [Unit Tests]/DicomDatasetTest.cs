﻿// Copyright (c) 2012-2015 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

namespace Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class DicomDatasetTest
    {
        #region Unit tests

        [Fact]
        public void Add_OtherDoubleElement_Succeeds()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.DoubleFloatPixelData, 3.45);
            Assert.IsType(typeof(DicomOtherDouble), dataset.First());
        }

        [Fact]
        public void Add_OtherDoubleElementWithMultipleDoubles_Succeeds()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.DoubleFloatPixelData, 3.45, 6.78, 9.01);
            Assert.IsType(typeof(DicomOtherDouble), dataset.First());
            Assert.Equal(3, dataset.Get<double[]>(DicomTag.DoubleFloatPixelData).Length);
        }

        [Fact]
        public void Add_UnlimitedCharactersElement_Succeeds()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.LongCodeValue, "abc");
            Assert.IsType(typeof(DicomUnlimitedCharacters), dataset.First());
            Assert.Equal("abc", dataset.Get<string>(DicomTag.LongCodeValue));
        }

        [Fact]
        public void Add_UnlimitedCharactersElementWithMultipleStrings_Succeeds()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.LongCodeValue, "a", "b", "c");
            Assert.IsType(typeof(DicomUnlimitedCharacters), dataset.First());
            Assert.Equal("c", dataset.Get<string>(DicomTag.LongCodeValue, 2));
        }

        [Fact]
        public void Add_UniversalResourceElement_Succeeds()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.URNCodeValue, "abc");
            Assert.IsType(typeof(DicomUniversalResource), dataset.First());
            Assert.Equal("abc", dataset.Get<string>(DicomTag.URNCodeValue));
        }

        [Fact]
        public void Add_UniversalResourceElementWithMultipleStrings_OnlyFirstValueIsUsed()
        {
            var dataset = new DicomDataset();
            dataset.Add(DicomTag.URNCodeValue, "a", "b", "c");
            Assert.IsType(typeof(DicomUniversalResource), dataset.First());

            var data = dataset.Get<string[]>(DicomTag.URNCodeValue);
            Assert.Equal(1, data.Length);
            Assert.Equal("a", data.First());
        }

        [Fact]
        public void Add_PersonName_MultipleNames_YieldsMultipleValues()
        {
            var dataset = new DicomDataset();
            dataset.Add(
                DicomTag.PerformingPhysicianName,
                "Gustafsson^Anders^L",
                "Yates^Ian",
                "Desouky^Hesham",
                "Horn^Chris");

            var data = dataset.Get<string[]>(DicomTag.PerformingPhysicianName);
            Assert.Equal(4, data.Length);
            Assert.Equal("Desouky^Hesham", data[2]);
        }

        [Theory]
        [MemberData("MultiVMStringTags")]
        public void Add_MultiVMStringTags_YieldsMultipleValues(DicomTag tag, string[] values, Type expectedType)
        {
            var dataset = new DicomDataset();
            dataset.Add(tag, values);

            Assert.IsType(expectedType, dataset.First());

            var data = dataset.Get<string[]>(tag);
            Assert.Equal(values.Length, data.Length);
            Assert.Equal(values.Last(), data.Last());
        }

        #endregion

        #region Support data

        public static IEnumerable<object[]> MultiVMStringTags
        {
            get
            {
                yield return
                    new object[]
                        {
                            DicomTag.ReferencedFrameNumber, new[] { "3", "5", "8" },
                            typeof(DicomIntegerString)
                        };
                yield return
                    new object[]
                        {
                            DicomTag.EventElapsedTimes, new[] { "3.2", "5.8", "8.7" },
                            typeof(DicomDecimalString)
                        };
                yield return
                new object[]
                        {
                            DicomTag.PatientTelephoneNumbers, new[] { "0271-22117", "070-669 5073", "0270-11204" },
                            typeof(DicomShortString)
                        };
                yield return
                new object[]
                        {
                            DicomTag.EventTimerNames, new[] { "a", "b", "c", "e", "f" },
                            typeof(DicomLongString)
                        };
                yield return
                new object[]
                        {
                            DicomTag.ConsultingPhysicianName, new[] { "a", "b", "c", "e", "f" },
                            typeof(DicomPersonName)
                        };
                yield return
                new object[]
                        {
                            DicomTag.SOPClassesSupported, new[] { "1.2.3", "4.5.6", "7.8.8.9" },
                            typeof(DicomUniqueIdentifier)
                        };
            }
        }

        #endregion
    }
}