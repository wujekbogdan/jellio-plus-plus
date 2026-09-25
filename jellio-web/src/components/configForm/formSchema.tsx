import { z } from 'zod';
import { catalogSortOrders } from '@/components/configForm/catalogSortOrder';

export const formSchema = z.object({
  serverName: z.string(),
  libraries: z.array(
    z.object({
      key: z.string(),
      name: z.string(),
      type: z.string(),
    }),
  ),
  sortOrders: z.record(z.string(), z.enum(catalogSortOrders)).default({}),
  jellyseerrEnabled: z.boolean().default(false),
  jellyseerrUrl: z.string().url().or(z.literal('')).default(''),
  jellyseerrApiKey: z.string().default(''),
  publicBaseUrl: z.string().url().or(z.literal('')).default(''),
});

export type ConfigFormType = z.input<typeof formSchema>;
