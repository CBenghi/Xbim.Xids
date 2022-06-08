// generated running xbim.xids.generator
using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class SchemaInfo
	{
		

		public static Dictionary<string, IfcMeasureInfo> IfcMeasures = new()
		{
			{ "AmountOfSubstance", new IfcMeasureInfo("AmountOfSubstance", "IfcAmountOfSubstanceMeasure", "Amount of substance", "mole", "mol", "(0, 0, 0, 0, 0, 1, 0)", new[] { "Ifc2x3.MeasureResource.IfcAmountOfSubstanceMeasure", "Ifc4.MeasureResource.IfcAmountOfSubstanceMeasure" }) },
			{ "AreaDensity", new IfcMeasureInfo("AreaDensity", "IfcAreaDensityMeasure", "Area density", "", "Kg/m2", "(-2, 1, 0, 0, 0, 0, 0)", new[] { "Ifc4.MeasureResource.IfcAreaDensityMeasure" }) },
			{ "Area", new IfcMeasureInfo("Area", "IfcAreaMeasure", "Area", "square meter", "m2", "(2, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcAreaMeasure", "Ifc4.MeasureResource.IfcAreaMeasure" }) },
			{ "DynamicViscosity", new IfcMeasureInfo("DynamicViscosity", "IfcDynamicViscosityMeasure", "Dynamic viscosity", "", "Pa s", "(-1, 1, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcDynamicViscosityMeasure", "Ifc4.MeasureResource.IfcDynamicViscosityMeasure" }) },
			{ "ElectricCapacitance", new IfcMeasureInfo("ElectricCapacitance", "IfcElectricCapacitanceMeasure", "Electric capacitance", "farad", "F", "(-2, 1, 4, 1, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricCapacitanceMeasure", "Ifc4.MeasureResource.IfcElectricCapacitanceMeasure" }) },
			{ "ElectricCharge", new IfcMeasureInfo("ElectricCharge", "IfcElectricChargeMeasure", "Electric charge", "coulomb", "C", "(0, 0, 1, 1, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricChargeMeasure", "Ifc4.MeasureResource.IfcElectricChargeMeasure" }) },
			{ "ElectricConductance", new IfcMeasureInfo("ElectricConductance", "IfcElectricConductanceMeasure", "Electric conductance", "siemens", "S", "(-2, -1, 3, 2, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricConductanceMeasure", "Ifc4.MeasureResource.IfcElectricConductanceMeasure" }) },
			{ "ElectricCurrent", new IfcMeasureInfo("ElectricCurrent", "IfcElectricCurrentMeasure", "Electric current", "ampere", "A", "(0, 0, 0, 1, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricCurrentMeasure", "Ifc4.MeasureResource.IfcElectricCurrentMeasure" }) },
			{ "ElectricResistance", new IfcMeasureInfo("ElectricResistance", "IfcElectricResistanceMeasure", "Electric resistance", "ohm", "Ω", "(2, 1, -3, -2, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricResistanceMeasure", "Ifc4.MeasureResource.IfcElectricResistanceMeasure" }) },
			{ "ElectricVoltage", new IfcMeasureInfo("ElectricVoltage", "IfcElectricVoltageMeasure", "Electric voltage", "volt", "V", "(2, 1, -3, -1, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcElectricVoltageMeasure", "Ifc4.MeasureResource.IfcElectricVoltageMeasure" }) },
			{ "Energy", new IfcMeasureInfo("Energy", "IfcEnergyMeasure", "Energy", "joule", "J", "(2, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcEnergyMeasure", "Ifc4.MeasureResource.IfcEnergyMeasure" }) },
			{ "Force", new IfcMeasureInfo("Force", "IfcForceMeasure", "Force", "newton", "N", "(1, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcForceMeasure", "Ifc4.MeasureResource.IfcForceMeasure" }) },
			{ "Frequency", new IfcMeasureInfo("Frequency", "IfcFrequencyMeasure", "Frequency", "hertz", "Hz", "(0, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcFrequencyMeasure", "Ifc4.MeasureResource.IfcFrequencyMeasure" }) },
			{ "HeatFluxDensity", new IfcMeasureInfo("HeatFluxDensity", "IfcHeatFluxDensityMeasure", "Heat flux density", "", "W/m2", "(0, 1, -3, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcHeatFluxDensityMeasure", "Ifc4.MeasureResource.IfcHeatFluxDensityMeasure" }) },
			{ "Heating", new IfcMeasureInfo("Heating", "IfcHeatingValueMeasure", "Heating", "", "J/K", "(2, 1, -2, 0, -1, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcHeatingValueMeasure", "Ifc4.MeasureResource.IfcHeatingValueMeasure" }) },
			{ "Illuminance", new IfcMeasureInfo("Illuminance", "IfcIlluminanceMeasure", "Illuminance", "lux", "lx", "(-2, 0, 0, 0, 0, 0, 1)", new[] { "Ifc2x3.MeasureResource.IfcIlluminanceMeasure", "Ifc4.MeasureResource.IfcIlluminanceMeasure" }) },
			{ "IonConcentration", new IfcMeasureInfo("IonConcentration", "IfcIonConcentrationMeasure", "Ion concentration measure", "", "mol/m3", "(-3, 1, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcIonConcentrationMeasure", "Ifc4.MeasureResource.IfcIonConcentrationMeasure" }) },
			{ "IsoThermalMoistureCapacity", new IfcMeasureInfo("IsoThermalMoistureCapacity", "IfcIsothermalMoistureCapacityMeasure", "Iso thermal moisture capacity", "", "m3/Kg", "(3, -1, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcIsothermalMoistureCapacityMeasure", "Ifc4.MeasureResource.IfcIsothermalMoistureCapacityMeasure" }) },
			{ "Length", new IfcMeasureInfo("Length", "IfcLengthMeasure", "Length", "meter", "m", "(1, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcLengthMeasure", "Ifc4.MeasureResource.IfcLengthMeasure" }) },
			{ "Speed", new IfcMeasureInfo("Speed", "IfcLinearVelocityMeasure", "Speed", "", "m/s", "(1, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcLinearVelocityMeasure", "Ifc4.MeasureResource.IfcLinearVelocityMeasure" }) },
			{ "LuminousFlux", new IfcMeasureInfo("LuminousFlux", "IfcLuminousFluxMeasure", "Luminous flux", "Lumen", "lm", "(0, 0, 0, 0, 0, 0, 1)", new[] { "Ifc2x3.MeasureResource.IfcLuminousFluxMeasure", "Ifc4.MeasureResource.IfcLuminousFluxMeasure" }) },
			{ "LuminousIntensity", new IfcMeasureInfo("LuminousIntensity", "IfcLuminousIntensityMeasure", "Luminous intensity", "candela", "cd", "(0, 0, 0, 0, 0, 0, 1)", new[] { "Ifc2x3.MeasureResource.IfcLuminousIntensityMeasure", "Ifc4.MeasureResource.IfcLuminousIntensityMeasure" }) },
			{ "MassDensity", new IfcMeasureInfo("MassDensity", "IfcMassDensityMeasure", "Mass density", "", "Kg/m3", "(-3, 1, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMassDensityMeasure", "Ifc4.MeasureResource.IfcMassDensityMeasure" }) },
			{ "MassFlowRate", new IfcMeasureInfo("MassFlowRate", "IfcMassFlowRateMeasure", "Mass flow rate", "", "Kg/s", "(0, 1, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMassFlowRateMeasure", "Ifc4.MeasureResource.IfcMassFlowRateMeasure" }) },
			{ "Mass", new IfcMeasureInfo("Mass", "IfcMassMeasure", "Mass", "kilogram", "Kg", "(0, 1, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMassMeasure", "Ifc4.MeasureResource.IfcMassMeasure" }) },
			{ "MassPerLength", new IfcMeasureInfo("MassPerLength", "IfcMassPerLengthMeasure", "Mass per length", "", "Kg/m", "(-1, 1, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMassPerLengthMeasure", "Ifc4.MeasureResource.IfcMassPerLengthMeasure" }) },
			{ "ModulusOfElasticity", new IfcMeasureInfo("ModulusOfElasticity", "IfcModulusOfElasticityMeasure", "Modulus of elasticity", "", "N/m2", "(-1, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcModulusOfElasticityMeasure", "Ifc4.MeasureResource.IfcModulusOfElasticityMeasure" }) },
			{ "MoistureDiffusivity", new IfcMeasureInfo("MoistureDiffusivity", "IfcMoistureDiffusivityMeasure", "Moisture diffusivity", "", "m3/s", "(3, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMoistureDiffusivityMeasure", "Ifc4.MeasureResource.IfcMoistureDiffusivityMeasure" }) },
			{ "MolecularWeight", new IfcMeasureInfo("MolecularWeight", "IfcMolecularWeightMeasure", "Molecular weight", "", "Kg/mol", "(0, 1, 0, 0, 0, -1, 0)", new[] { "Ifc2x3.MeasureResource.IfcMolecularWeightMeasure", "Ifc4.MeasureResource.IfcMolecularWeightMeasure" }) },
			{ "MomentOfInertia", new IfcMeasureInfo("MomentOfInertia", "IfcMomentOfInertiaMeasure", "Moment of inertia", "", "m4", "(4, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcMomentOfInertiaMeasure", "Ifc4.MeasureResource.IfcMomentOfInertiaMeasure" }) },
			{ "PH", new IfcMeasureInfo("PH", "IfcPHMeasure", "PH", "", "PH", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcPHMeasure", "Ifc4.MeasureResource.IfcPHMeasure" }) },
			{ "PlanarForce", new IfcMeasureInfo("PlanarForce", "IfcPlanarForceMeasure", "Planar force", "", "Pa", "(-1, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcPlanarForceMeasure", "Ifc4.MeasureResource.IfcPlanarForceMeasure" }) },
			{ "Angle", new IfcMeasureInfo("Angle", "IfcPlaneAngleMeasure", "Angle", "radian", "rad", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcPlaneAngleMeasure", "Ifc4.MeasureResource.IfcPlaneAngleMeasure" }) },
			{ "PlaneAngle", new IfcMeasureInfo("PlaneAngle", "IfcPlaneAngleMeasure", "Plane angle", "radian", "rad", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcPlaneAngleMeasure", "Ifc4.MeasureResource.IfcPlaneAngleMeasure" }) },
			{ "Power", new IfcMeasureInfo("Power", "IfcPowerMeasure", "Power", "watt", "W", "(2, 1, -3, 0, 0, 0, 0", new[] { "Ifc2x3.MeasureResource.IfcPowerMeasure", "Ifc4.MeasureResource.IfcPowerMeasure" }) },
			{ "Pressure", new IfcMeasureInfo("Pressure", "IfcPressureMeasure", "Pressure", "pascal", "Pa", "(-1, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcPressureMeasure", "Ifc4.MeasureResource.IfcPressureMeasure" }) },
			{ "RadioActivity", new IfcMeasureInfo("RadioActivity", "IfcRadioActivityMeasure", "Radio activity", "Becqurel", "Bq", "(0, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcRadioActivityMeasure", "Ifc4.MeasureResource.IfcRadioActivityMeasure" }) },
			{ "Ratio", new IfcMeasureInfo("Ratio", "IfcRatioMeasure", "Ratio", "Percent", "%", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcRatioMeasure", "Ifc4.MeasureResource.IfcRatioMeasure" }) },
			{ "RotationalFrequency", new IfcMeasureInfo("RotationalFrequency", "IfcRotationalFrequencyMeasure", "Rotational frequency", "hertz", "Hz", "(0, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcRotationalFrequencyMeasure", "Ifc4.MeasureResource.IfcRotationalFrequencyMeasure" }) },
			{ "SectionModulus", new IfcMeasureInfo("SectionModulus", "IfcSectionModulusMeasure", "Section modulus", "", "m3", "(3, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcSectionModulusMeasure", "Ifc4.MeasureResource.IfcSectionModulusMeasure" }) },
			{ "SoundPower", new IfcMeasureInfo("SoundPower", "IfcSoundPowerMeasure", "Sound power", "decibel", "db", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcSoundPowerMeasure", "Ifc4.MeasureResource.IfcSoundPowerMeasure" }) },
			{ "SoundPressure", new IfcMeasureInfo("SoundPressure", "IfcSoundPressureMeasure", "Sound pressure", "decibel", "db", "(0, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcSoundPressureMeasure", "Ifc4.MeasureResource.IfcSoundPressureMeasure" }) },
			{ "SpecificHeatCapacity", new IfcMeasureInfo("SpecificHeatCapacity", "IfcSpecificHeatCapacityMeasure", "Specific heat capacity", "", "J/Kg K", "(2, 0, -2, 0, -1, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcSpecificHeatCapacityMeasure", "Ifc4.MeasureResource.IfcSpecificHeatCapacityMeasure" }) },
			{ "TemperatureRateOfChange", new IfcMeasureInfo("TemperatureRateOfChange", "IfcTemperatureRateOfChangeMeasure", "Temperature rate of change", "", "K/s", "(0, 0, -1, 0, 1, 0, 0)", new[] { "Ifc4.MeasureResource.IfcTemperatureRateOfChangeMeasure" }) },
			{ "ThermalConductivity", new IfcMeasureInfo("ThermalConductivity", "IfcThermalConductivityMeasure", "Thermal conductivity", "", "W/m K", "(1, 1, -3, 0, -1, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcThermalConductivityMeasure", "Ifc4.MeasureResource.IfcThermalConductivityMeasure" }) },
			{ "Temperature", new IfcMeasureInfo("Temperature", "IfcThermodynamicTemperatureMeasure", "Temperature", "kelvin", "K", "(0, 0, 0, 0, 1, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcThermodynamicTemperatureMeasure", "Ifc4.MeasureResource.IfcThermodynamicTemperatureMeasure" }) },
			{ "Time", new IfcMeasureInfo("Time", "IfcTimeMeasure", "Time", "second", "s", "(0, 0, 1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcTimeMeasure", "Ifc4.MeasureResource.IfcTimeMeasure" }) },
			{ "Torque", new IfcMeasureInfo("Torque", "IfcTorqueMeasure", "Torque", "", "N m", "(2, 1, -2, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcTorqueMeasure", "Ifc4.MeasureResource.IfcTorqueMeasure" }) },
			{ "VaporPermeability", new IfcMeasureInfo("VaporPermeability", "IfcVaporPermeabilityMeasure", "Vapor permeability", "", "Kg / s m Pa", "(0, 0, 1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcVaporPermeabilityMeasure", "Ifc4.MeasureResource.IfcVaporPermeabilityMeasure" }) },
			{ "Volume", new IfcMeasureInfo("Volume", "IfcVolumeMeasure", "Volume", "cubic meter", "m3", "(3, 0, 0, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcVolumeMeasure", "Ifc4.MeasureResource.IfcVolumeMeasure" }) },
			{ "VolumetricFlowRate", new IfcMeasureInfo("VolumetricFlowRate", "IfcVolumetricFlowRateMeasure", "Volumetric flow rate", "", "m3/s", "(3, 0, -1, 0, 0, 0, 0)", new[] { "Ifc2x3.MeasureResource.IfcVolumetricFlowRateMeasure", "Ifc4.MeasureResource.IfcVolumetricFlowRateMeasure" }) },
			{ "String", new IfcMeasureInfo("String", "", "", "", "", "", new[] { "" }) },
			{ "Number", new IfcMeasureInfo("Number", "", "", "", "", "", new[] { "" }) },
		};
	}
}
