import { OrderedProduct } from './order.model';
import { Product } from './product.model';

export interface ConfiguratorSelection {
  [categoryName: string]: string;
}

export interface Incompatibility {
  rule: string;
  categoryA: string;
  categoryB: string;
  message: string;
}

export interface ConfiguratorValidationResult {
  isValid: boolean;
  issues: Incompatibility[];
  resolvedItems: OrderedProduct[];
  hasIntegratedGraphics: boolean;
}

export interface ConfiguratorCategory {
  id: string;
  name: string;
  fields: { id: string; name: string; typeCode: number }[];
}

export interface ConfiguratorPartOption extends Product {
  isCompatible: boolean;
}
