export const catalogSortOrders = [
  'Name',
  'RecentlyAdded',
  'ReleaseDate',
] as const;

type CatalogSortOrder = (typeof catalogSortOrders)[number];

export const defaultCatalogSortOrder =
  'RecentlyAdded' satisfies CatalogSortOrder;
