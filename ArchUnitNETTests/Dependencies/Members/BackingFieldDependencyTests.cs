/*
 * Copyright 2019 TNG Technology Consulting GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNETTests.Fluent;
using Xunit;

namespace ArchUnitNETTests.Dependencies.Members
{
    public class BackingFieldDependencyTests
    {
        private readonly Architecture _architecture = StaticTestArchitectures.ArchUnitNETTestArchitecture;

        private readonly PropertyMember _backedProperty;
        private readonly FieldMember _expectedBackingField;
        private readonly PropertyMember _implicitlyBackedAutoProperty;
        private readonly FieldMember _lambdaBackingField;
        private readonly PropertyMember _lambdaBackedProperty;

        public BackingFieldDependencyTests()
        {
            var backingFieldClass = _architecture.GetClassOfType(typeof(BackingFieldExamples));
            _backedProperty = backingFieldClass
                .GetPropertyMembersWithName(nameof(BackingFieldExamples.FieldPropertyPair)).First();
            _expectedBackingField = backingFieldClass.GetFieldMembersWithName("_fieldPropertyPair").First();
            _implicitlyBackedAutoProperty = backingFieldClass
                .GetPropertyMembersWithName(nameof(BackingFieldExamples.AutoProperty)).First();
            _lambdaBackedProperty = backingFieldClass
                .GetPropertyMembersWithName(nameof(BackingFieldExamples.LambdaFieldPropertyPair)).First();
            _lambdaBackingField = backingFieldClass.GetFieldMembersWithName("_lambdaFieldPropertyPair").First();
        }

        [Fact]
        public void ExplicitBackingFieldAssigned()
        {
            Assert.Equal(_expectedBackingField, _backedProperty.BackingField);
        }

        [Fact]
        public void LambdaBackingFieldAssignment()
        {
            Assert.Equal(_lambdaBackingField, _lambdaBackedProperty.BackingField);
        }

        [Fact]
        public void BackedPropertyInheritsBackingFieldDependencies()
        {
            _backedProperty.BackingField.MemberDependencies
                .ForEach(memberDependency => Assert.Contains(memberDependency, _backedProperty.MemberDependencies));
        }

        [Fact]
        public void ImplicitBackingFieldStubAssigned()
        {
            // BackingFields are not assigned if the backing field is auto-generated by the compiler
            Assert.Null(_implicitlyBackedAutoProperty.BackingField);
        }
    }
    
    public class BackingFieldExamples
    {
        public PropertyType AutoProperty { get; set; }

        private ChildField _fieldPropertyPair;
        private PropertyType _lambdaFieldPropertyPair;

        public PropertyType FieldPropertyPair
        {
            get { return _fieldPropertyPair; }
            set { _fieldPropertyPair = (ChildField) value; }
        }

        public PropertyType LambdaFieldPropertyPair
        {
            get => _lambdaFieldPropertyPair;
            set => _lambdaFieldPropertyPair = value;
        }
    }
}