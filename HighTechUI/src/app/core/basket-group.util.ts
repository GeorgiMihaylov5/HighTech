import { OrderedProduct } from '../models/order.model';

export interface BasketBuild {
    buildId: string;
    items: OrderedProduct[];
    total: number;
}

export interface GroupedBasket {
    items: OrderedProduct[];
    builds: BasketBuild[];
}

export function groupOrderedProducts(orderedProducts: OrderedProduct[]): GroupedBasket {
    const basket = orderedProducts ?? [];
    const regularProducts = basket.filter(op => !op.buildId);
    const buildMap = new Map<string, OrderedProduct[]>();

    for (const op of basket) {
        if (op.buildId) {
            const existing = buildMap.get(op.buildId) ?? [];
            existing.push(op);
            buildMap.set(op.buildId, existing);
        }
    }

    const builds = Array.from(buildMap.entries()).map(([buildId, items]) => ({
        buildId,
        items,
        total: items.reduce((sum, item) => sum + (item.orderedPrice ?? item.product?.price ?? 0) * item.count, 0)
    }));

    return { items: regularProducts, builds };
}
