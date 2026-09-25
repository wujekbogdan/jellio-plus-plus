import type { ConfigFormType } from '@/components/configForm/formSchema';
import { stripTrailingSlash } from '@/lib/utils';
import type { SaveConfigData } from '@/services/backendService';

export const toServerConfig = (values: ConfigFormType) => ({
  jellyseerrEnabled: values.jellyseerrEnabled ?? false,
  jellyseerrUrl: stripTrailingSlash(values.jellyseerrUrl ?? ''),
  jellyseerrApiKey: values.jellyseerrApiKey ?? '',
  publicBaseUrl: stripTrailingSlash(values.publicBaseUrl ?? ''),
  selectedLibraries: values.libraries.map((library) =>
    library.key.replace(/-/g, ''),
  ),
  catalogSortOrders: Object.entries(values.sortOrders ?? {}).map(
    ([libraryId, sortOrder]) => ({ libraryId, sortOrder }),
  ),
});

export const sortOrdersFromServer = (
  catalogSortOrders: NonNullable<SaveConfigData['catalogSortOrders']>,
) =>
  Object.fromEntries(
    catalogSortOrders.map(({ libraryId, sortOrder }) => [libraryId, sortOrder]),
  );
