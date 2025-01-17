import Measure from "./measure";
import MeasureWithDatum from "./measureWithDatum";
import Rheometer from "./rheometer";

export default interface Fluid {
  uid: string;
  type: string;
  locationSample: string;
  dTim: string;
  md: MeasureWithDatum;
  tvd: MeasureWithDatum;
  presBopRating: Measure;
  mudClass: string;
  density: Measure;
  visFunnel: Measure;
  tempVis: Measure;
  pv: Measure;
  yp: Measure;
  gel10Sec: Measure;
  gel10Min: Measure;
  gel30Min: Measure;
  filterCakeLtlp: Measure;
  filtrateLtlp: Measure;
  tempHthp: Measure;
  presHthp: Measure;
  filtrateHthp: Measure;
  filterCakeHthp: Measure;
  solidsPc: Measure;
  waterPc: Measure;
  oilPc: Measure;
  sandPc: Measure;
  solidsLowGravPc: Measure;
  solidsCalcPc: Measure;
  baritePc: Measure;
  lcm: Measure;
  mbt: Measure;
  ph: string;
  tempPh: Measure;
  pm: Measure;
  pmFiltrate: Measure;
  mf: Measure;
  alkalinityP1: Measure;
  alkalinityP2: Measure;
  chloride: Measure;
  calcium: Measure;
  magnesium: Measure;
  potassium: Measure;
  rheometers: Rheometer[];
  brinePc: Measure;
  lime: Measure;
  electStab: Measure;
  calciumChloride: Measure;
  company: string;
  solidsHiGravPc: Measure;
  polymer: Measure;
  polyType: string;
  solCorPc: Measure;
  oilCtg: Measure;
  hardnessCa: Measure;
  sulfide: Measure;
  comments: string;
}
