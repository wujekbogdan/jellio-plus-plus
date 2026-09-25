import { useId, type FC } from 'react';
import { useWatch, type UseFormReturn } from 'react-hook-form';
import {
  catalogSortOrders,
  defaultCatalogSortOrder,
} from '@/components/configForm/catalogSortOrder';
import type { ConfigFormType } from '@/components/configForm/formSchema.tsx';
import { Checkbox } from '@/components/ui/checkbox.tsx';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
} from '@/components/ui/form.tsx';
import { Select } from '@/components/ui/select.tsx';
import type { Library } from '@/types';

const sortOrderLabels = {
  Name: 'A-Z',
  RecentlyAdded: 'Recently added',
  ReleaseDate: 'Release date',
} satisfies Record<(typeof catalogSortOrders)[number], string>;

interface Props {
  form: UseFormReturn<ConfigFormType>;
  serverName: string;
  library: Library;
}

export const LibrarySection: FC<Props> = ({ form, serverName, library }) => {
  const enabledLibraries = useWatch({
    control: form.control,
    name: 'libraries',
  });
  const isEnabled = enabledLibraries.some(({ key }) => key === library.key);
  const headingId = useId();

  return (
    <div role="group" aria-labelledby={headingId}>
      <h3 id={headingId} className="mb-2 text-sm font-medium">
        {`${library.name} | ${serverName}`}
      </h3>
      <div className="flex flex-row items-center gap-6">
        <FormField
          control={form.control}
          name="libraries"
          render={({ field }) => (
            <FormItem className="flex flex-row items-center gap-3 space-y-0">
              <FormLabel className="font-normal">Enabled</FormLabel>
              <FormControl>
                <Checkbox
                  checked={isEnabled}
                  onCheckedChange={(checked) =>
                    field.onChange(
                      checked
                        ? [...field.value, library]
                        : field.value.filter(({ key }) => key !== library.key),
                    )
                  }
                />
              </FormControl>
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name={`sortOrders.${library.key}`}
          render={({ field }) => (
            <FormItem className="flex flex-row items-center gap-3 space-y-0">
              <FormLabel className="font-normal">Sort order</FormLabel>
              <FormControl>
                <Select
                  className="w-44"
                  name={field.name}
                  value={field.value ?? defaultCatalogSortOrder}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  disabled={!isEnabled}
                >
                  {catalogSortOrders.map((sortOrder) => (
                    <option key={sortOrder} value={sortOrder}>
                      {sortOrderLabels[sortOrder]}
                    </option>
                  ))}
                </Select>
              </FormControl>
            </FormItem>
          )}
        />
      </div>
    </div>
  );
};
